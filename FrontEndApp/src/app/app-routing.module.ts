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

const routes: Routes = [
	{ path: '', component: HomeComponent },
	{ path: 'login', component: LoginComponent },
	{ path: 'signup', component: SignUpComponent },
	{ path: 'customers', component: CustomersComponent, canActivate: [AuthGuard]},
	{ path: 'customers/create', component: CreateCustomerComponent, canActivate: [AuthGuard]},
	{ path: 'invoices', component: InvoicesComponent, canActivate: [AuthGuard]},
	{ path: 'invoices/import', component: ImportInvoiceComponent, canActivate: [AuthGuard]},
	{ path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
