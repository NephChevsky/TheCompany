import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { User } from '../_models/user';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService
{
	private currentUserSubject: BehaviorSubject<User>;
	public currentUser: Observable<User>;

	constructor(private http: HttpClient)
	{
		this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser') || "{}"));
		this.currentUser = this.currentUserSubject.asObservable();
	}

	public get currentUserValue(): User
	{
		return this.currentUserSubject.value;
	}

	storeUser(user: User)
	{
		localStorage.setItem('currentUser', JSON.stringify(user));
		this.currentUserSubject.next(user);
		return user;
	}

	register(user: User)
	{
		return this.http.post<User>(environment.baseUrl + `User/Register`, user)
			.pipe(map(user =>
			{
				return this.storeUser(user);
			}));
	}

	login(user: User)
	{
		return this.http.post<User>(environment.baseUrl + `User/Login`, user)
				.pipe(map(user =>
				{
					return this.storeUser(user);
				}));
	}

	logout()
	{
		localStorage.removeItem('currentUser');
		this.currentUserSubject.next(new User());
	}
}
