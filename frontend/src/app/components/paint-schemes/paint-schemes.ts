import { Component, inject, OnInit, signal } from '@angular/core';
import { PaintSchemeService, PaintScheme } from '../../services/paint-scheme.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-paint-schemes',
  standalone: true,
  imports: [],
  templateUrl: './paint-schemes.html',
  styleUrl: './paint-schemes.css',
})
export class PaintSchemesComponent implements OnInit {
  private paintSchemeService = inject(PaintSchemeService);
  private authService = inject(AuthService);

  paintSchemes = signal<PaintScheme[]>([]);
  currentUserId = this.authService.userId;

  ngOnInit() {
    this.loadSchemes();
  }

  loadSchemes() {
    this.paintSchemeService.getPaintSchemes().subscribe({
      next: (schemes) => this.paintSchemes.set(schemes),
      error: (err) => console.error('Error loading schemes:', err)
    });
  }
}
