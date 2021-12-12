import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowLineItemComponent } from './show-line-item.component';

describe('ShowLineItemComponent', () => {
  let component: ShowLineItemComponent;
  let fixture: ComponentFixture<ShowLineItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowLineItemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowLineItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
