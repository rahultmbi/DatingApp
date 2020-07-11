import { AlertifyService } from './../_service/alertify.service';
import { AuthService } from './../_service/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;

  constructor(public authService: AuthService, private alertifyService: AlertifyService,
              private router: Router ) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photourl => this.photoUrl = photourl);
  }

  login() {
     this.authService.login(this.model).subscribe(next => {
       this.alertifyService.success('Logged in successfully');
     },
     error => {
      this.alertifyService.error(error);
     }, () => {
      this.router.navigate(['/members']);
     });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertifyService.success('loged out successfully');
    this.router.navigate(['home']);
  }

}
