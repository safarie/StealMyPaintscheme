import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme, Step } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';

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

  paintSchemes = signal<PaintScheme[]>([]);
  currentUserId = this.authService.userId;

  ownSchemes = computed(() =>
    this.paintSchemes().filter(s => s.userId === this.currentUserId())
  );

  otherSchemes = computed(() =>
    this.paintSchemes().filter(s => s.userId !== this.currentUserId())
  );

  // We kunnen de sortering ook in de frontend doen voor de zekerheid,
  // maar de API doet het al. We laten het nu zo.

  name = '';
  description = '';
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
      steps: this.steps
    };

    this.paintSchemeService.addPaintScheme(newScheme).subscribe({
      next: () => {
        this.loadSchemes();
        this.name = '';
        this.description = '';
        this.steps = [];
      },
      error: (err) => console.error('Error adding scheme:', err)
    });
  }
}
