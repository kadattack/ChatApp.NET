import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {map} from "rxjs/operators";
import {User} from "../_models/user";
import {ReplaySubject} from "rxjs";
import {environment} from "../../environments/environment";

// services stay active for whole application life
// components get destroyed after changing url on website

@Injectable({
  // Adds to app.module.ts services=[]
  providedIn: 'root'
})
export class AccountService {
  baseUrl= environment.apiUrl;
  // Placeholder for the stored User that we can fetch with subscribe
  private currentUserSource = new ReplaySubject<User>(1)
  currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  login(model:any){
    return this.http.post(this.baseUrl + "account/login", model).pipe(
      map((response:User) =>{
       const user = response;
       if (user){
         localStorage.setItem('user',JSON.stringify(user));
         this.currentUserSource.next(user);
       }

    }))

  }

  setCurrentUser(user:User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

  }
  register(model:any){
    return this.http.post(this.baseUrl + "account/register", model)
  }
}
