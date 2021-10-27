import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './_services/auth.guard';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './signup/signup.component';
import { CustomersComponent } from './customers/customers.component';
import { CreateCustomerComponent } from './customers/create-customer/create-customer.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { ImportInvoiceComponent } from './invoices/import-invoice/import-invoice.component';
import { InvoiceExtractionComponent } from './settings/invoiceextraction/invoiceextraction.component';
import { ShowInvoiceComponent } from './invoices/show-invoice/show-invoice.component';
import { SettingsComponent } from './settings/settings.component';

const routes: Routes = [
	{ path: '', component: HomeComponent },
	{ path: 'Login', component: LoginComponent },
	{ path: 'SignUp', component: SignUpComponent },
	{ path: 'Customers', component: CustomersComponent, canActivate: [AuthGuard]},
	{ path: 'Customers/Create', component: CreateCustomerComponent, canActivate: [AuthGuard]},
	{ path: 'Invoices', component: InvoicesComponent, canActivate: [AuthGuard]},
	{ path: 'Invoices/Import', component: ImportInvoiceComponent, canActivate: [AuthGuard]},
	{ path: 'Invoices/Show/:id', component: ShowInvoiceComponent, canActivate: [AuthGuard]},
	{ path: 'Settings', component: SettingsComponent, canActivate: [AuthGuard]},
	{ path: 'Settings/InvoiceExtraction', component: InvoiceExtractionComponent, canActivate: [AuthGuard]},
	{ path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
