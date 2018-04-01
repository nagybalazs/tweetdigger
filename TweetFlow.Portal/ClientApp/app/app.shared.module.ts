import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { ChannelComponent } from './components/channel/channel.component';
import { TweetComponent } from './components/tweet/tweet.component';
import { HomeComponent } from './components/home/home.component';
import { SignupComponent } from './components/signup/signup.component';

import { ChannelService, AccountService } from './services/services';
import { HttpClientModule } from '@angular/common/http';

import { ClickOutsideModule } from 'ng-click-outside';

@NgModule({
    providers: [
        ChannelService,
        AccountService
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        ChannelComponent,
        TweetComponent,
        SignupComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        HttpClientModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: '**', redirectTo: 'home' }
        ]),
        ClickOutsideModule
    ]
})
export class AppModuleShared {
}
