import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import { UserService } from '../_service/user.service';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { User } from '../_models/user';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;
  constructor(private authService: AuthService, private userService: UserService,
              private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
     // console.log(data['users'].result);
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
  loadUsers() {
    this.userService
    .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam)
    .subscribe(
      (res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
