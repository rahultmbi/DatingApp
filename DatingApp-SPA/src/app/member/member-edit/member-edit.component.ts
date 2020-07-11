import { AlertifyService } from './../../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_service/user.service';
import { AuthService } from 'src/app/_service/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
       $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute, private aleretify: AlertifyService,
              private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }

  updateUser(id: number) {
    console.log(this.user);
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
    this.aleretify.success('User data updated successfully.');
    this.editForm.reset(this.user);
    }, error => {
      this.aleretify.error(error);
    });
  }

  UpdateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}
