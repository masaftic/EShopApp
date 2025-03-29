import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  constructor(private authService: AuthService, private router: Router) { }

  // Function to handle logout
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Function to check if the user is logged in
  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }
}
