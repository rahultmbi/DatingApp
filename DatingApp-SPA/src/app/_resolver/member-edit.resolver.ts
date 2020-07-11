import { AlertifyService } from '../_service/alertify.service';
import { UserService } from '../_service/user.service';
import { User } from '../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_service/auth.service';

@Injectable()

export class MemberEditResolver implements Resolve<User>
{
    constructor(private userService: UserService, private router: Router, private authService: AuthService,
                private alertify: AlertifyService) {}
 
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving your data.');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }

}

