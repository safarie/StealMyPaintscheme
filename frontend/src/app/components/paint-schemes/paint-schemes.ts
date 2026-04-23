import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-paint-schemes',
  imports: [FormsModule],
  templateUrl: './paint-schemes.html',
  styleUrl: './paint-schemes.css',
})
export class PaintSchemesComponent {
  name = signal('');
  description = signal('');

  onSubmit() {
    console.log('Nieuw schema:', {
      name: this.name(),
      description: this.description()
    });
    // Reset form
    this.name.set('');
    this.description.set('');
  }
}
