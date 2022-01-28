import { Component, OnInit } from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  model:any = {}
  wrongCred: boolean;
  constructor(private accountService : AccountService, private router :Router ) {

  }
  ngOnInit(): void {
  }
  login(){
    this.accountService.login(this.model).subscribe(res => {
      this.router.navigateByUrl("");

    }, error => {
      this.wrongCred = true;
      console.log(error)})
  }

}
