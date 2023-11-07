import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
model: any ={}


constructor(public accountService: AccountService) {
 }
ngOnInit(){

}

login(){
  this.accountService.login(this.model).subscribe(response =>{
    console.log(response);
    console.error();
  })
}
logout(){
  this.accountService.logout();
}


}
