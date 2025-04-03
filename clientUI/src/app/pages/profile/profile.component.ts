import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  user: any;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        console.log(user);
        this.user = user;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
