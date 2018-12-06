import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../chat.service';
import { first } from 'rxjs/operators';
import { ChatMessage } from '../ChatMessage';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
 
  model: any = {};
  ngOnInIt() {
  
  }

  loading = false;
  constructor(private router: Router, private chatservice: ChatService, private chatMsg: ChatMessage) {

  }


  onSubmit() {
    this.loading = true;
    this.chatservice.AddUserDetails(this.model)
      .pipe(first())
      .subscribe(
        data => {
          debugger;
          this.chatMsg = data;
          let key = 'User';
          let myObj = { id: this.chatMsg.id, userName: this.chatMsg.userName, nickName: this.chatMsg.nickName };
          localStorage.setItem(key, JSON.stringify(myObj));
          this.router.navigate(['/chat']);
        },
        error => {
          if (error.status == "404")
            alert("Invalid UserName and Password");

          console.log(error + ' while posting data ' + this.model);
          this.loading = false;
        });
  }
}
