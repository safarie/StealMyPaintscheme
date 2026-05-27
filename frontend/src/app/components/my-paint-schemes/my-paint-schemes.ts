import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme, Step } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';
import { InventoryService, InventoryItem } from '../../services/inventory.service';

@Component({
  selector: 'app-my-paint-schemes',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './my-paint-schemes.html',
  styleUrl: './my-paint-schemes.css',
})
export class MyPaintSchemesComponent implements OnInit {
  private paintSchemeService = inject(PaintSchemeService);
  private authService = inject(AuthService);
  private inventoryService = inject(InventoryService);

  paintSchemes = signal<PaintScheme[]>([]);
  inventory = signal<InventoryItem[]>([]);
  isLoggedIn = this.authService.isLoggedIn;
  currentUserId = this.authService.userId;
  selectedScheme = signal<PaintScheme | null>(null);

  ownSchemes = computed(() =>
    this.paintSchemes().filter(s =>
      s.userId === this.currentUserId() &&
      !s.isStolen
    )
  );

  stolenSchemes = computed(() =>
    this.paintSchemes().filter(s =>
      s.userId === this.currentUserId() &&
      s.isStolen
    )
  );

  name = '';
  description = '';
  tagsInput = '';
  steps: Step[] = [];
  editingSchemeId = signal<number | null>(null);

  ngOnInit() {
    this.loadSchemes();
    if (this.isLoggedIn()) {
      this.loadInventory();
    }
  }

  loadSchemes() {
    this.paintSchemeService.getPaintSchemes().subscribe({
      next: (schemes) => this.paintSchemes.set(schemes),
      error: (err) => console.error('Error loading schemes:', err)
    });
  }

  loadInventory() {
    this.inventoryService.getInventory().subscribe({
      next: (items) => this.inventory.set(items),
      error: (err) => console.error('Error loading inventory:', err)
    });
  }

  addStep() {
    this.steps.push({
      where: '',
      colour: '',
      paintingTechnique: '',
      paintId: undefined
    });
  }

  onColourInput(index: number) {
    const step = this.steps[index];
    const selectedItem = this.inventory().find(item => item.paint?.name === step.colour);
    if (selectedItem && selectedItem.paint) {
      step.paintId = selectedItem.paint.id;
    } else {
      step.paintId = undefined;
    }
  }

  removeStep(index: number) {
    this.steps.splice(index, 1);
  }

  editScheme(scheme: PaintScheme) {
    this.editingSchemeId.set(scheme.id || null);
    this.name = scheme.name;
    this.description = scheme.description || '';
    this.tagsInput = scheme.tags ? scheme.tags.join(', ') : '';
    // Deep copy steps to avoid direct modification
    this.steps = scheme.steps.map(s => ({ ...s }));

    // Scroll naar formulier
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.editingSchemeId.set(null);
    this.name = '';
    this.description = '';
    this.tagsInput = '';
    this.steps = [];
  }

  onSubmit(event: Event) {
    event.preventDefault();

    if (!this.name || !this.description) {
      alert('Naam en beschrijving zijn verplicht.');
      return;
    }

    if (this.steps.length === 0) {
      alert('Voeg minimaal één stap toe.');
      return;
    }

    for (const step of this.steps) {
      if (!step.where || !step.colour || !step.paintingTechnique) {
        alert('Alle velden in elke stap (Waar, Kleur, Techniek) moeten zijn ingevuld.');
        return;
      }
    }

    const schemeData: PaintScheme = {
      name: this.name,
      description: this.description,
      tags: this.tagsInput ? this.tagsInput.split(',').map(t => t.trim()).filter(t => t !== '') : [],
      steps: this.steps.map(s => {
        const { id, ...rest } = s; // Verwijder id bij nieuwe stappen of bij update (backend vervangt ze)
        return rest as Step;
      })
    };

    const editId = this.editingSchemeId();
    if (editId) {
      this.paintSchemeService.updatePaintScheme(editId, schemeData).subscribe({
        next: () => {
          this.loadSchemes();
          this.cancelEdit();
        },
        error: (err) => console.error('Error updating scheme:', err)
      });
    } else {
      this.paintSchemeService.addPaintScheme(schemeData).subscribe({
        next: () => {
          this.loadSchemes();
          this.cancelEdit();
        },
        error: (err) => console.error('Error adding scheme:', err)
      });
    }
  }

  deleteScheme(id: number | undefined) {
    if (!id) return;

    if (confirm('Weet je zeker dat je dit verfschema wilt verwijderen?')) {
      this.paintSchemeService.deletePaintScheme(id).subscribe({
        next: () => {
          this.loadSchemes();
        },
        error: (err) => console.error('Error deleting scheme:', err)
      });
    }
  }

  viewDetails(scheme: PaintScheme) {
    this.selectedScheme.set(scheme);
  }

  closeDetails() {
    this.selectedScheme.set(null);
  }

  hasPaintInInventory(paintId: number | undefined): boolean {
    if (!paintId) return false;
    return this.inventory().some(item => item.paintId === paintId);
  }

  getMissingPaintsCount(scheme: PaintScheme): number {
    const uniquePaintIds = new Set(
      scheme.steps
        .map(s => s.paintId)
        .filter((id): id is number => id !== undefined)
    );

    let missingCount = 0;
    uniquePaintIds.forEach(id => {
      if (!this.hasPaintInInventory(id)) {
        missingCount++;
      }
    });

    return missingCount;
  }
}
