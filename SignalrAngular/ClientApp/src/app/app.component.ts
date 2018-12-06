import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';
  nickName: string;
  model: any[];

  constructor(private _router: Router) {
    debugger;
    if (localStorage.length > 0) {
      let item = JSON.parse(localStorage.getItem('User'));
      if (item != undefined) {
        this.nickName = item.userName;
        this._router.navigate(['/chat']);
      }
    } else {
      this._router.navigate(['/']);
    }
  }

  Logout() {
    let key = 'User';
    localStorage.removeItem(key);
    localStorage.clear();
    this._router.navigate(['/']);
  }
}
