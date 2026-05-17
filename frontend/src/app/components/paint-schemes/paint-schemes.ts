import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaintSchemeService, PaintScheme } from '../../services/paint-scheme.service';
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
  searchTerm = signal<string>('');

  filteredSchemes = computed(() => {
    const term = this.searchTerm().toLowerCase();
    const schemes = this.paintSchemes();

    if (!term) return schemes;

    return schemes.filter(s =>
      s.name.toLowerCase().includes(term) ||
      (s.description && s.description.toLowerCase().includes(term)) ||
      (s.tags && s.tags.some(t => t.toLowerCase().includes(term)))
    );
  });

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
