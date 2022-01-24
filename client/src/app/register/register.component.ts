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
    let regExplarge = /[A-Z]/;
    let regExpnumb = /[0-9]/


    if (this.model.password != this.model.confirm_password){
      this.errorMsg = "Confirm Password does not match with Password."
    } else if ( !regExplarge.test(this.model.password) || !regExpnumb.test(this.model.password) ){
      this.errorMsg = "Password must contain at least one upper case letter and at least one number."
    } else if (this.model.password.length < 4){
      this.errorMsg = "Password must must be at least 4 characters long."
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
