import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClearAllComponent } from './clear-all.component';

describe('ClearAllComponent', () => {
  let component: ClearAllComponent;
  let fixture: ComponentFixture<ClearAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ClearAllComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ClearAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
