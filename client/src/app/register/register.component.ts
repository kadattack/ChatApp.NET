import { Component, OnInit } from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Router} from "@angular/router";
import {thBeLocale} from "ngx-bootstrap/chronos";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  errorMsg = ""
  model:any = {}
  constructor(private accountService : AccountService, private router: Router ) {

  }
  ngOnInit(): void {
  }

  register(){
    console.log(this.model)
    if (this.model.password != this.model.confirm_password){
      this.errorMsg = "Confirm Password does not match with Password."
    } else {
      this.accountService.register(this.model).subscribe(res => {
        this.router.navigateByUrl("login");
        console.log(res)
      }, error => {
        this.errorMsg = error.error
        console.log(error)})
    }

  }

}
