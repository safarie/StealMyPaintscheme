import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme, Step } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';

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

  paintSchemes = signal<PaintScheme[]>([]);
  currentUserId = this.authService.userId;
  selectedScheme = signal<PaintScheme | null>(null);

  ownSchemes = computed(() =>
    this.paintSchemes().filter(s => s.userId === this.currentUserId() && !s.name.endsWith('(Stolen)'))
  );

  stolenSchemes = computed(() =>
    this.paintSchemes().filter(s => s.userId === this.currentUserId() && s.name.endsWith('(Stolen)'))
  );

  name = '';
  description = '';
  tagsInput = '';
  steps: Step[] = [];

  ngOnInit() {
    this.loadSchemes();
  }

  loadSchemes() {
    this.paintSchemeService.getPaintSchemes().subscribe({
      next: (schemes) => this.paintSchemes.set(schemes),
      error: (err) => console.error('Error loading schemes:', err)
    });
  }

  addStep() {
    this.steps.push({
      where: '',
      colour: '',
      paintingTechnique: ''
    });
  }

  removeStep(index: number) {
    this.steps.splice(index, 1);
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

    const newScheme: PaintScheme = {
      name: this.name,
      description: this.description,
      tags: this.tagsInput ? this.tagsInput.split(',').map(t => t.trim()).filter(t => t !== '') : [],
      steps: this.steps
    };

    this.paintSchemeService.addPaintScheme(newScheme).subscribe({
      next: () => {
        this.loadSchemes();
        this.name = '';
        this.description = '';
        this.tagsInput = '';
        this.steps = [];
      },
      error: (err) => console.error('Error adding scheme:', err)
    });
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
}
