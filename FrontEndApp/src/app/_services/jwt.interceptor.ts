import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

import { UserService } from './user.service';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class JwtInterceptor implements HttpInterceptor
{
	constructor(private userService: UserService, private router: Router, private toastr: ToastrService) { }

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>
  {
		const currentUser = this.userService.currentUserValue;
		if (currentUser && currentUser.token)
		{
			request = request.clone({
				setHeaders: {
					Authorization: `Bearer ${currentUser.token}`
				}	
			});
		}

		return next.handle(request).pipe(catchError(err =>
		{
			debugger;
			if (err.status === 401)
			{
				this.userService.logout();
				this.router.navigate(['/Login']);
			}
			else if (err.status === 0 && err.name === "HttpErrorResponse")
			{
				this.toastr.error("Couldn't reach server");
			}
			return throwError(err);
		}))
	}
}
