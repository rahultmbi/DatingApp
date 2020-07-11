import { PaginatedResult } from './../_models/Pagination';
import { User } from './../_models/user';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable, pipe } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  baseUrl = environment.apiUrl;

  getUsers(page?, itemsPerPage?, userParam?, likesParam?): Observable<PaginatedResult<User[]>> {
    console.log(page, itemsPerPage);
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParam != null) {
      params = params.append('minAge', userParam.minAge);
      params = params.append('maxAge', userParam.maxAge);
      params = params.append('gender', userParam.gender);
      params = params.append('orderBy', userParam.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http.get<User[]>(this.baseUrl + 'users' ,
    { observe: 'response', params })
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(id: number, userid: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/photos/' + userid + '/setMain', {});
  }

  deletePhoto(userid: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userid + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {} )
  }
}




