import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme, Step } from '../../services/paint-scheme.service';

@Component({
  selector: 'app-paint-schemes',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './paint-schemes.html',
  styleUrl: './paint-schemes.css',
})
export class PaintSchemesComponent implements OnInit {
  private paintSchemeService = inject(PaintSchemeService);

  paintSchemes = signal<PaintScheme[]>([]);
  name = '';
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

    const newScheme: PaintScheme = {
      name: this.name,
      steps: this.steps
    };

    this.paintSchemeService.addPaintScheme(newScheme).subscribe({
      next: () => {
        this.loadSchemes();
        this.name = '';
        this.steps = [];
      },
      error: (err) => console.error('Error adding scheme:', err)
    });
  }
}
