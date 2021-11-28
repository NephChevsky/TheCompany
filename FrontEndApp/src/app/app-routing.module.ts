import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './_services/auth.guard';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './signup/signup.component';
import { CustomersComponent } from './customers/customers.component';
import { CreateCustomerComponent } from './customers/create-customer/create-customer.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { ShowInvoiceComponent } from './invoices/show-invoice/show-invoice.component';
import { ImportInvoiceComponent } from './invoices/import-invoice/import-invoice.component';
import { InvoiceExtractionComponent } from './settings/invoiceextraction/invoiceextraction.component';
import { SettingsComponent } from './settings/settings.component';
import { ClearAllComponent } from './settings/clear-all/clear-all.component';
import { InvoiceComponent } from './settings/invoice/invoice.component';
import { AddComponent } from './settings/additional-fields/add/add.component';
import { CustomerComponent } from './settings/customer/customer.component';

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
	{ path: 'Settings/Customer', component: CustomerComponent, canActivate: [AuthGuard]},
	{ path: 'Settings/Invoice', component: InvoiceComponent, canActivate: [AuthGuard]},
	{ path: 'Settings/InvoiceExtraction', component: InvoiceExtractionComponent, canActivate: [AuthGuard]},
	{ path: 'Settings/ClearAll', component: ClearAllComponent, canActivate: [AuthGuard]},
	{ path: 'AdditionalFields/Add/:dataSource', component: AddComponent, canActivate: [AuthGuard]},
	{ path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
