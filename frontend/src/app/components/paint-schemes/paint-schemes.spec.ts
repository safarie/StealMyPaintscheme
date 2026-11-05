import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { PaintSchemesComponent } from './paint-schemes';

describe('PaintSchemesComponent', () => {
  let component: PaintSchemesComponent;
  let fixture: ComponentFixture<PaintSchemesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaintSchemesComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(PaintSchemesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
