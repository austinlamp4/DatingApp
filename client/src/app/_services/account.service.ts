import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
  
export class AccountService {
  baseUrl = 'https://localhost:5000/api/';
  private currentUserSource = new BehaviorSubject<User | null>(null); //This observable can be User or null
  currentUser$ = this.currentUserSource.asObservable(); //$ indicates this is an observable

  constructor(private http: HttpClient) { }
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  setCurrentUser(user: User) { //Will be used from component to set the information inside AccountService
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
