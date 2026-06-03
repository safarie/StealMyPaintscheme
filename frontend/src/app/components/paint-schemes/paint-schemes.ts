import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';
import { InventoryService, InventoryItem } from '../../services/inventory.service';

@Component({
  selector: 'app-paint-schemes',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './paint-schemes.html',
  styleUrl: './paint-schemes.css',
})
export class PaintSchemesComponent implements OnInit {
  private paintSchemeService = inject(PaintSchemeService);
  private authService = inject(AuthService);
  private inventoryService = inject(InventoryService);

  paintSchemes = signal<PaintScheme[]>([]);
  inventory = signal<InventoryItem[]>([]);
  isLoggedIn = this.authService.isLoggedIn;
  currentUserId = this.authService.userId;
  searchTerm = signal<string>('');
  selectedScheme = signal<PaintScheme | null>(null);

  filteredSchemes = computed(() => {
    const term = this.searchTerm().toLowerCase();
    const schemes = this.paintSchemes();
    const currentId = this.currentUserId();

    // Filter alle "stolen" schema's uit de algemene lijst
    const originalSchemes = schemes.filter(s => !s.isStolen);

    if (!term) return originalSchemes;

    return originalSchemes.filter(s =>
      s.name.toLowerCase().includes(term) ||
      (s.description && s.description.toLowerCase().includes(term)) ||
      (s.tags && s.tags.some(t => t.toLowerCase().includes(term)))
    );
  });

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

  formatRGB(rgb: string | undefined): string {
    if (!rgb) return '';
    if (rgb.startsWith('#')) return rgb;
    if (rgb.includes(',')) {
      return `rgb(${rgb})`;
    }
    return rgb;
  }

  viewDetails(scheme: PaintScheme) {
    this.selectedScheme.set(scheme);
  }

  stealScheme(scheme: PaintScheme) {
    // Maak een kopie van het schema zonder de originele ID's om het als nieuw op te slaan
    const stolenScheme: PaintScheme = {
      name: scheme.name,
      description: scheme.description,
      isStolen: true,
      tags: scheme.tags ? [...scheme.tags] : [],
      steps: scheme.steps.map(step => ({
        where: step.where,
        colour: step.colour,
        paintingTechnique: step.paintingTechnique,
        paintId: step.paintId
      }))
    };

    this.paintSchemeService.addPaintScheme(stolenScheme).subscribe({
      next: () => {
        alert('Scheme stolen successfully!');
        this.loadSchemes();
      },
      error: (err) => console.error('Error stealing scheme:', err)
    });
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
