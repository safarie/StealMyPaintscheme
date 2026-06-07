import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme, Step } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';
import { InventoryService, InventoryItem, GlobalPaint } from '../../services/inventory.service';

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
  globalPaints = signal<GlobalPaint[]>([]);
  isLoggedIn = this.authService.isLoggedIn;
  currentUserId = this.authService.userId;
  selectedScheme = signal<PaintScheme | null>(null);

  groupedSteps = computed(() => {
    const scheme = this.selectedScheme();
    if (!scheme) return [];

    const groups: { where: string; steps: Step[] }[] = [];
    scheme.steps.forEach(step => {
      let group = groups.find(g => g.where.toLowerCase() === step.where.toLowerCase());
      if (!group) {
        group = { where: step.where, steps: [] };
        groups.push(group);
      }
      group.steps.push(step);
    });
    return groups;
  });

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
  selectedFile: File | null = null;
  imageUrl = '';
  editingSchemeId = signal<number | null>(null);

  // Hulpselectie voor formulier om stappen te groeperen op onderdeel ('where')
  parts: { name: string; steps: Step[] }[] = [];

  ngOnInit() {
    this.loadSchemes();
    this.loadGlobalPaints();
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

  loadGlobalPaints() {
    this.inventoryService.getGlobalPaints().subscribe({
      next: (paints) => this.globalPaints.set(paints),
      error: (err) => console.error('Error loading global paints:', err)
    });
  }

  addPart() {
    this.parts.push({
      name: '',
      steps: [{
        where: '',
        colour: '',
        paintingTechnique: '',
        paintId: undefined
      }]
    });
  }

  removePart(index: number) {
    this.parts.splice(index, 1);
  }

  addStepToPart(partIndex: number) {
    this.parts[partIndex].steps.push({
      where: this.parts[partIndex].name,
      colour: '',
      paintingTechnique: '',
      paintId: undefined
    });
  }

  removeStepFromPart(partIndex: number, stepIndex: number) {
    this.parts[partIndex].steps.splice(stepIndex, 1);
    if (this.parts[partIndex].steps.length === 0) {
      this.removePart(partIndex);
    }
  }

  formatRGB(rgb: string | undefined): string {
    if (!rgb) return '';
    if (rgb.startsWith('#')) return rgb;
    if (rgb.includes(',')) {
      return `rgb(${rgb})`;
    }
    return rgb;
  }

  onColourInput(partIndex: number, stepIndex: number) {
    const step = this.parts[partIndex].steps[stepIndex];

    // Check of de input de vorm "Naam (Type)" heeft, wat gebeurt bij een selectie uit de datalist
    const selectionMatch = step.colour.match(/^(.+) \((.+)\)$/);

    if (selectionMatch) {
      const name = selectionMatch[1];
      const type = selectionMatch[2];

      const exactMatch = this.globalPaints().find(p =>
        p.name.toLowerCase() === name.toLowerCase() &&
        p.type.toLowerCase() === type.toLowerCase()
      );

      if (exactMatch) {
        step.colour = exactMatch.name;
        step.paintId = exactMatch.id;
        // Optioneel: techniek invullen als die leeg is? In inventory wordt maker en type ingevuld.
        return;
      }
    }

    // Zoek in eigen inventory
    const selectedItem = this.inventory().find(item => item.paint?.name.toLowerCase() === step.colour.toLowerCase());
    if (selectedItem && selectedItem.paint) {
      step.paintId = selectedItem.paint.id;
    } else {
      // Zoek in global paints
      const globalMatch = this.globalPaints().find(p => p.name.toLowerCase() === step.colour.toLowerCase());
      if (globalMatch) {
        step.paintId = globalMatch.id;
      } else {
        step.paintId = undefined;
      }
    }
  }

  removeStep(index: number) {
    // Wordt niet meer direct gebruikt, vervangen door removeStepFromPart
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  editScheme(scheme: PaintScheme) {
    this.editingSchemeId.set(scheme.id || null);
    this.name = scheme.name;
    this.description = scheme.description || '';
    this.tagsInput = scheme.tags ? scheme.tags.join(', ') : '';
    this.imageUrl = scheme.imageUrl || '';

    // Groepeer stappen voor de UI
    const groups: { name: string; steps: Step[] }[] = [];
    if (scheme.steps && scheme.steps.length > 0) {
      scheme.steps.forEach(step => {
        let group = groups.find(g => g.name.toLowerCase() === (step.where || '').toLowerCase());
        if (!group) {
          group = { name: step.where, steps: [] };
          groups.push(group);
        }
        group.steps.push({ ...step });
      });
    } else {
      // Fallback als er geen stappen zijn (zou niet moeten gebeuren)
      groups.push({
        name: '',
        steps: [{
          where: '',
          colour: '',
          paintingTechnique: '',
          paintId: undefined
        }]
      });
    }
    this.parts = groups;

    // Scroll naar formulier
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.editingSchemeId.set(null);
    this.name = '';
    this.description = '';
    this.tagsInput = '';
    this.steps = [];
    this.parts = [];
    this.selectedFile = null;
    this.imageUrl = '';
  }

  onSubmit(event: Event) {
    event.preventDefault();

    if (!this.name || !this.description) {
      alert('Naam en beschrijving zijn verplicht.');
      return;
    }

    if (this.parts.length === 0) {
      alert('Voeg minimaal één onderdeel toe.');
      return;
    }

    // Valideer parts en steps
    for (const part of this.parts) {
      if (!part.name) {
        alert('Alle onderdelen moeten een naam hebben.');
        return;
      }
      if (part.steps.length === 0) {
        alert(`Onderdeel "${part.name}" heeft geen stappen.`);
        return;
      }
      for (const step of part.steps) {
        if (!step.colour || !step.paintingTechnique) {
          alert(`Niet alle velden zijn ingevuld voor een stap in "${part.name}".`);
          return;
        }
        // Zorg dat 'where' overeenkomt met de part naam
        step.where = part.name;
      }
    }

    if (this.selectedFile) {
      this.paintSchemeService.uploadImage(this.selectedFile).subscribe({
        next: (response) => {
          this.saveScheme(response.imageUrl);
        },
        error: (err) => {
          console.error('Error uploading image:', err);
          alert('Fout bij het uploaden van de afbeelding.');
        }
      });
    } else {
      this.saveScheme(this.imageUrl);
    }
  }

  saveScheme(imageUrl: string) {
    // Platstaan van parts naar steps voor API
    const flattenedSteps: Step[] = [];
    this.parts.forEach(part => {
      part.steps.forEach(step => {
        const { id, ...rest } = step;
        flattenedSteps.push({
          ...rest,
          where: part.name
        } as Step);
      });
    });

    const schemeData: PaintScheme = {
      name: this.name,
      description: this.description,
      imageUrl: imageUrl,
      tags: this.tagsInput ? this.tagsInput.split(',').map(t => t.trim()).filter(t => t !== '') : [],
      steps: flattenedSteps
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
