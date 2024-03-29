import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { AuthGuard } from './_guards/auth.guard';
import { Routes, CanDeactivate } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './member/member-list/member-list.component';
import { MemberDetailResolver } from './_resolver/member-detail.resolver';
import { MemberListResolver } from './_resolver/member-list.resolver';
import { MemberEditResolver } from './_resolver/member-edit.resolver';
import { PreventUnsavedChnages } from './_guards/prevent-unsaved-chnages.guard';
import { ListsResolver } from './_resolver/lists.resolver';

export const appRoutes: Routes = [
    { path: 'home', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver}},
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver}},
            { path: 'member/edit', component: MemberEditComponent,
                    resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChnages]},
            { path: 'messages', component: MessagesComponent},
            { path: 'lists', component: ListsComponent, resolve: { users: ListsResolver}},
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full'}
 ];