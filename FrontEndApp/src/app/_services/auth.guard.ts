import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate
{
	constructor(
		private router: Router,
		private userService: UserService
	) { }

	canActivate()
	{
		const currentUser = this.userService.currentUserValue;
		if (currentUser)
		{
			return true;
		}
		
		this.userService.logout();
		this.router.navigate(['/Login']);
		return false;
	}
}
