import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService, InventoryItem, Paint } from '../../services/inventory.service';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory.html',
  styleUrl: './inventory.css',
})
export class InventoryComponent implements OnInit {
  private inventoryService = inject(InventoryService);

  inventoryItems = signal<InventoryItem[]>([]);
  showForm = false;
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Form fields
  newName = '';
  newType = '';
  newBrand = '';
  newQuantity = 1;

  ngOnInit() {
    this.loadInventory();
  }

  loadInventory() {
    this.inventoryService.getInventory().subscribe({
      next: (items) => this.inventoryItems.set(items),
      error: (err) => console.error('Error loading inventory:', err)
    });
  }

  toggleForm() {
    this.showForm = !this.showForm;
  }

  onSave(event: Event) {
    event.preventDefault();
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (!this.newName || !this.newType || !this.newBrand) {
      this.errorMessage.set('Vul alle verplichte velden in.');
      return;
    }

    const newPaint: Paint = {
      name: this.newName,
      type: this.newType,
      maker: this.newBrand
    };

    // First create the paint, then create the inventory item
    this.inventoryService.addPaint(newPaint).subscribe({
      next: (paint) => {
        if (paint.id) {
          const newItem: InventoryItem = {
            paintId: paint.id,
            quantity: this.newQuantity
          };
          this.inventoryService.addInventoryItem(newItem).subscribe({
            next: () => {
              this.loadInventory();
              this.resetForm();
              this.showForm = false;
              this.successMessage.set('Paint successfully added to inventory.');
            },
            error: (err) => {
              console.error('Error adding paint to inventory:', err);
              this.errorMessage.set('Error adding paint to inventory.');
            }
          });
        }
      },
      error: (err) => {
        console.error('Error adding paint:', err);
        this.errorMessage.set('Error adding paint to inventory.');
      }
    });
  }

  private resetForm() {
    this.newName = '';
    this.newType = '';
    this.newBrand = '';
    this.newQuantity = 1;
  }
}
