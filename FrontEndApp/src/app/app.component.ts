import { Component } from '@angular/core';
import { UserService } from './_services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent
{
	title: string = 'TheCompany';
	guest: boolean = true;
	menu: any[] = [
		{name: "Home", route: ""},
		{name: "Customers", route: "Customers"},
		{name: "Invoices", route: "Invoices"},
		{name: "Settings", route: "Settings", childrens: [
			{
				name: "Invoice Extraction",
				route: "Settings/InvoiceExtraction"
			},
			{
				name: "Clear All",
				route: "Settings/ClearAll"
			}]
		}
	];

	constructor(
		private userService: UserService,
		private router: Router
	)
	{
		userService.currentUser.subscribe(value =>
		{
			this.guest = (value.id === undefined || value.id === '');
		});
	}

	logout()
	{
		this.userService.logout();
		this.router.navigate(['/Login']);
	}
}
