import { Component } from '@angular/core';
import { UserService } from './_services/user.service';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

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
		{ name: "Menu.Home", route: "" },
		{ name: "Menu.Customers", route: "Customers" },
		{ name: "Menu.Invoices", route: "Invoices" },
		{ name: "Menu.LineItems", route: "LineItems" },
		{
			name: "Menu.Settings.Main", route: "Settings", childrens: [
				{
					name: "Menu.Settings.CompanyInformation",
					route: "Settings/Company"
				},
				{
					name: "Menu.Settings.Customer",
					route: "Settings/Customer"
				},
				{
					name: "Menu.Settings.Invoice",
					route: "Settings/Invoice"
				},
				{
					name: "Menu.Settings.InvoiceExtraction",
					route: "Settings/InvoiceExtraction"
				},
				{
					name: "Menu.Settings.ClearAll",
					route: "Settings/ClearAll"
				}]
		}
	];
	language: string;

	constructor(
		private userService: UserService,
		private router: Router,
		public translate: TranslateService
	)
	{
		translate.addLangs(['en', 'fr']);
		translate.setDefaultLang('en');
		var tmp = localStorage.getItem('selectedLanguage');
		if (tmp)
		{
			this.language = tmp;
		}
		else
		{
			const browserLang = translate.getBrowserLang();
			this.language = browserLang.match(/en|fr/) ? browserLang : 'en'
		}
		translate.use(this.language);

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
