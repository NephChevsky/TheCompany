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
		this.router.navigate(['/login']);
	}
}
