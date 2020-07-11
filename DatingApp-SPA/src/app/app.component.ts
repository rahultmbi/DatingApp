import { AuthService } from './_service/auth.service';
import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
jwtHelper = new JwtHelperService();

constructor(private authService: AuthService) {}

ngOnInit() {
    const token = localStorage.getItem('token');
    const currentUser = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (currentUser) {
      this.authService.currentUser = currentUser;
      this.authService.changeMemberPhoto(currentUser.photourl);
    }
  }
}
