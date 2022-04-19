import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EMPTY, Subject } from 'rxjs';
import { finalize, switchMap, takeUntil } from 'rxjs/operators';
import { AUTH_ID_CODE, AuthService } from '../../common/services/auth.service';
import { SnackbarService } from '../../common/services/snackbar.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent implements OnInit, OnDestroy {
  public authInProcess = false;
  private destroy$ = new Subject();

  constructor(
    public authService: AuthService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private snackbarService: SnackbarService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams
      .pipe(
        switchMap((params) => {
          const isLoginProcess = params.hasOwnProperty(AUTH_ID_CODE) && params[AUTH_ID_CODE].length;

          if (isLoginProcess) {
            this.authInProcess = true;
            this.authService.authCode = params[AUTH_ID_CODE];

            return this.authService.getAccessToken();
          } else if (this.authService.accessToken) {
            return this.router.navigate(['']);
          } else {
            return EMPTY;
          }
        }),
        finalize(() => {
          this.authInProcess = false;
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: (res) => {
          if (res) {
            this.router.navigate(['']);
          }
        },
        error: (err) => {
          this.router.navigate(['login']);
          this.snackbarService.showErrorSnackbar();
          throw new Error(err); // show message
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
