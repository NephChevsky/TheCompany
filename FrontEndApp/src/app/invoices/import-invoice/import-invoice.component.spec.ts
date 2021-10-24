import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportInvoiceComponent } from './import-invoice.component';

describe('ImportInvoiceComponent', () => {
  let component: ImportInvoiceComponent;
  let fixture: ComponentFixture<ImportInvoiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportInvoiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
