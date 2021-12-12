import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLineItemComponent } from './create-line-item.component';

describe('CreateLineItemComponent', () => {
  let component: CreateLineItemComponent;
  let fixture: ComponentFixture<CreateLineItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateLineItemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateLineItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
