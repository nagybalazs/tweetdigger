import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { ChannelComponent } from './components/channel/channel.component';
import { TweetComponent } from './components/tweet/tweet.component';
import { HomeComponent } from './components/home/home.component';

import { ChannelService } from './services/services';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
    providers: [
        ChannelService    ],
    declarations: [
        AppComponent,
        HomeComponent,
        ChannelComponent,
        TweetComponent
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
        ])
    ]
})
export class AppModuleShared {
}
