import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

baseurl = 'http://localhost:5000/api/auth/';

currentUser: User;

jwtHelper = new JwtHelperService();

decodedToken: any;

photoUrl =  new BehaviorSubject<string>('..//..//assets/user.jpg');

constructor(private http: HttpClient) { }
currentPhotoUrl = this.photoUrl.asObservable();

login(model: any) {
  return this.http.post(this.baseurl + 'login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
          
        }
      })
    );
}

register(user: User) {
  return this.http.post(this.baseurl + 'register', user);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

changeMemberPhoto(photoUrl: string) {
  this.photoUrl.next(photoUrl);
}
}
