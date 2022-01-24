import {Component, OnInit} from '@angular/core';
import {User} from "./_models/user";
import {AccountService} from "./_services/account.service";
import {environment} from "../environments/environment";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{


  title = 'client';

  constructor(private accountService: AccountService) {
  }
  setCurrentUser(){
    const user:User = JSON.parse(localStorage.getItem('user'))
    this.accountService.setCurrentUser(user)
  }

  ngOnInit(): void {
    this.setCurrentUser()
  }



}

