import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AngularMaterialModule } from './angular-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { SignUpComponent } from './signup/signup.component';
import { LoginComponent } from './login/login.component';
import { CdkTreeModule } from '@angular/cdk/tree';
import { JwtInterceptor } from './_services/jwt.interceptor';
import { CustomersComponent } from './customers/customers.component';
import { CreateCustomerComponent } from './customers/create-customer/create-customer.component';
import { ViewListComponent } from './_modules/view-list/view-list.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { ImportInvoiceComponent } from './invoices/import-invoice/import-invoice.component';
import { SettingsComponent } from './settings/settings.component';
import { InvoiceExtractionComponent } from './settings/invoiceextraction/invoiceextraction.component';
import { ShowInvoiceComponent } from './invoices/show-invoice/show-invoice.component';
import { ClearAllComponent } from './settings/clear-all/clear-all.component';
import { InvoiceComponent } from './settings/invoice/invoice.component';
import { AddComponent } from './settings/additional-fields/add/add.component';
import { CustomerComponent } from './settings/customer/customer.component';
import { PreviewComponent } from './_modules/preview/preview.component';
import { TextFieldComponent } from './_modules/fields/text-field/text-field.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    SignUpComponent,
    LoginComponent,
    CustomersComponent,
    CreateCustomerComponent,
    ViewListComponent,
    InvoicesComponent,
    ImportInvoiceComponent,
	SettingsComponent,
    InvoiceExtractionComponent,
    ShowInvoiceComponent,
    ClearAllComponent,
    InvoiceComponent,
    AddComponent,
    CustomerComponent,
    PreviewComponent,
    TextFieldComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    AngularMaterialModule,
    FormsModule,
    CdkTreeModule
  ],
  providers: [[{
    provide: HTTP_INTERCEPTORS,
    useClass: JwtInterceptor,
    multi: true
  }]],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
