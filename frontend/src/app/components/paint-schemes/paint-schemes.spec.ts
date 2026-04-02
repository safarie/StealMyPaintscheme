import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaintSchemes } from './paint-schemes';

describe('PaintSchemes', () => {
  let component: PaintSchemes;
  let fixture: ComponentFixture<PaintSchemes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaintSchemes],
    }).compileComponents();

    fixture = TestBed.createComponent(PaintSchemes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
