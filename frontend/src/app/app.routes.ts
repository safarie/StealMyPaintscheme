import { Routes } from '@angular/router';
import { LandingComponent } from './components/landing/landing';
import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';
import { PaintSchemesComponent } from './components/paint-schemes/paint-schemes';
import { InventoryComponent } from './components/inventory/inventory';

export const routes: Routes = [
  { path: '', component: LandingComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'paint-schemes', component: PaintSchemesComponent },
  { path: 'inventory', component: InventoryComponent },
  { path: '**', redirectTo: '' }
];
