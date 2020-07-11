import { AlertifyService } from './../_service/alertify.service';
import { UserService } from './../_service/user.service';
import { User } from './../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()

export class ListsResolver implements Resolve<User[]>
{
    pageNumber = 1;
    itemsPerPage = 5;
    likesParam = 'Likers';
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.itemsPerPage, null, this.likesParam).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data.');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}

