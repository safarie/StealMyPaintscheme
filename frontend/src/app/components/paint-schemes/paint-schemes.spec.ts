import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaintSchemesComponent } from './paint-schemes';

describe('PaintSchemesComponent', () => {
  let component: PaintSchemesComponent;
  let fixture: ComponentFixture<PaintSchemesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaintSchemesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PaintSchemesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
