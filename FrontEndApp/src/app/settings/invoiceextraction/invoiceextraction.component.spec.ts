import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceExtractionComponent } from './invoiceextraction.component';

describe('InvoiceExtractionComponent', () => {
  let component: InvoiceExtractionComponent;
  let fixture: ComponentFixture<InvoiceExtractionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InvoiceExtractionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceExtractionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
