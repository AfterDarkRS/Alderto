import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
  public loggedIn: boolean;
  public userImg: string;

  constructor(
    private readonly account: AccountService,
    private readonly userService: UserService) { }

  public ngOnInit() {
    this.userService.user$.subscribe(user => {
      this.loggedIn = !!user;

      if (user)
        this.userImg = user.avatar
          ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.jpg?size=64`
          : this.getDefaultAvatar(user.discriminator);
    });
  }

  public login() {
    this.account.signIn().subscribe();
  }

  public logout() {
    this.account.signOut().subscribe(() => {
      window.location.href = '/';
    });
  }

  private getDefaultAvatar(discriminator): string {
    switch (discriminator % 5) {
      case 1:
        return 'https://discordapp.com/assets/322c936a8c8be1b803cd94861bdfa868.png';
      case 2:
        return 'https://discordapp.com/assets/dd4dbc0016779df1378e7812eabaa04d.png';
      case 3:
        return 'https://discordapp.com/assets/0e291f67c9274a1abdddeb3fd919cbaa.png';
      case 4:
        return 'https://discordapp.com/assets/1cbd08c76f8af6dddce02c5138971129.png';
      default:
        return 'https://discordapp.com/assets/6debd47ed13483642cf09e832ed0bc1b.png';
    }
  }
}
