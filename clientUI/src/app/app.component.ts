import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from "./pages/footer/footer.component";
import { HeaderComponent } from './pages/header/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'clientUI';
}
