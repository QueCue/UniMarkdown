# How to Design an Attractive GitHub Profile Readme…

After shuffling through a lot of ideas about what should be my next blog post, I decided to go with a fairly simple one this time: **How to create a visually appealing GitHub Profile Readme.** You all knew this was coming right? Some of you might be thinking “Ugh, great, yet another blog on how to make an impressive GitHub profile 😑”. Well…yes. But just because something already exists doesn’t mean that you cannot create your unique version of it. Most people do make their profile readme’s interesting but what’s the difference between profiles that look good and the ones that are aesthetically pleasing?

So here we are, I am going to show you guys how to design a stellar GitHub Profile Readme and I’m going to use [**my profile**](https://github.com/thepiyushmalhotra) as an example. I’ll mention all the resources that I used for my profile and how you can customize it according to your style.

But before we get into the design and styling of the profile, many of you may ask “**What’s the reason for doing this?**” and “**Is it necessary?**”. To that, I would say it’s not necessary to style your profile and make it unique. There is no major downside to not creating a readme profile but there are definitely a lot of upsides to it.

Everyone used GitHub as a dumping ground for their projects, just using it simply for what it was designed which is source control, and collaborating with others on a project or contributing towards open source. But in 2020, when GitHub released this new feature of creating our very own unique readme profile, it provided a way for developers and artists alike to showcase their work professionally in the form of a “**personal resume**” on GitHub. That’s all it is, **a special repo that acts as a visually impressive portfolio for other developers and employers to check out.** So read on for some incredible tips on styling your GitHub Profile!

1.  The Very First Steps 🐤

***

To start designing your GitHub profile, we first have to create a new public repository. Click the “**+**” icon at the top right and choose “**_New Repository_**”. After that comes the critical step, **_make sure that the name of the repo is the same as your username_**. Refer to the example below:

Press enter or click to view image in full size

![Image 5](https://miro.medium.com/v2/resize:fit:700/1*iv5zkATlQzmU9R40V0lccQ.png)

[thepiyushmalhotra](https://github.com/thepiyushmalhotra)

GitHub will let you know that you have found a special repo whose README.md file can be customized. We want to keep this repo “**_public_**_”_ so that it can be displayed on your GitHub profile. You can provide a brief description of the repository (e.g. _“_**_My GitHub Profile_**_”)_ though this step is optional. After that, tick the checkbox _“_**_Add a README file_**_”_ and click on _“_**_Create repository_**_”._ Going ahead, we will alter this README file and personalize it.

Press enter or click to view image in full size

![Image 6](https://miro.medium.com/v2/resize:fit:700/1*TPz96UyU2jxi8Mkr7WvqWA.png)

GitHub Profile Settings

2.  A Unique Header ❄️

***

Your profile’s header is the first thing that people will observe so it must stand out from the other profiles. We want that initial “hook” that attracts the viewer. And for that to happen, my suggestion would be to avoid following the common design norms. For example, many developers use this layout for their “About” section:

### Hi there 👋

*   👂 My name is ...
    
*   👩 Pronouns: ...
    
*   🔭 I’m currently working on ...
    
*   🌱 I’m currently learning ...
    
*   🤝 I’m looking to collaborate on ...
    
*   🤔 I’m looking for help with ...
    
*   💬 Ask me about ...
    
*   📫 How to reach me: ...
    
*   ❤️ I love ...
    
*   ⚡ Fun fact: ... It’s completely fine to use this template though as long as you change other aspects of your profile. I went in another direction for creating the header and then added the “**_About Me_**” section after that. I’ll guide you along the way with the resources I used for that.
    

Press enter or click to view image in full size

![Image 7](https://miro.medium.com/v2/resize:fit:700/1*cxueL722-cuMCKlrfAWuwQ.gif)

[My GitHub’s Profile Header](https://github.com/thepiyushmalhotra)

*   The very first thing that you can see is that animated header with the text “Hey Everyone!” I used the [**capsule-render GitHub repo**](https://github.com/kyechan99/capsule-render) for this one. I came across this great resource while I was searching for ways to decorate your GitHub repo. You can add background images and text on top of them and also, who doesn’t love animations! It’s super simple to use and has been well documented on the Repo. Here is my configuration of the render.

![]([https://capsule-render.vercel.app/api?text=Hey](https://capsule-render.vercel.app/api?text=Hey) Everyone!🕹️&animation=fadeIn&type=waving&color=gradient&height=100)

\* After inserting a simple heading, to provide links to my various accounts like LinkedIn, Medium, Dev.to, and also my portfolio website, I wanted a minimalistic and textless way to do it. So, I decided to use icons. There are many online tools available that provide thousands of free icons to use. I used \[\*\*IconFinder\*\*\](https://www.iconfinder.com/)and personally loved it. There are many other popular options available that you can use like \[\*\*Shields.io\*\*\](https://shields.io/), \[\*\*markdown-badges\*\*\](https://github.com/Ileriayo/markdown-badges), \[\*\*vector-logo-zone\*\*\](https://www.vectorlogo.zone/index.html), \[\*\*simple-icons\*\*\](https://simpleicons.org/), etc.

The png source of the icon just needs to be imported into the _<\_\*\*\_img\_\*\*\_>_ tag as shown below:

 [![]([https://user-images.githubusercontent.com/46517096/166974368-9798f39f-1f46-499c-b14e-81f0a3f83a06.png](https://user-images.githubusercontent.com/46517096/166974368-9798f39f-1f46-499c-b14e-81f0a3f83a06.png))]([https://www.instagram.com/thepiyushmalhotra/]\(https://www.instagram.com/thepiyushmalhotra/\))\* Now comes the fun part, adding that glorious GIF! GIFS make our profile more \*\*dynamic and eye-catching\*\*. Honestly, you can put up any GIF you want. It can be a popular meme, a programming gif, an iconic scene from a movie or a tv show, or something that tells people a little bit about your hobbies. In my case, it’s anime so that’s what I went with. Popular gif sharing websites like \[\*\*Giphy\*\*\](https://giphy.com/)and \[\*\*Tenor\*\*\](https://tenor.com/) can be used to pull any gif you like and it works the same way as adding the icons, just copy the image address and paste it inside the “\*\*\_src\_\*\*” attribute of the \_<\_\*\*\_img\_\*\*\_>\_ tag.

3.  The “About Me” Section 👨‍💻

***

This is the section that I was talking about earlier where most developers use the template shown above. If you want to make your profile stand out, then I would suggest changing the design aspects of this section as well. I went ahead and used **YAML**format while editing the readme so that the information reads like \*\*_code_\*\*when you preview the profile.

Press enter or click to view image in full size

![Image 8](https://miro.medium.com/v2/resize:fit:700/1*_g1X-OaK3vLysA-tJrMMfA.png)

[My GitHub’s About Section](https://github.com/thepiyushmalhotra)

It adds a touch of professionalism and also as a bonus, looks neat! To display this format, just wrap your text as shown below and you’ll be good to go:

```yaml

* YOUR TEXT GOES HERE *
```

4.  Tools and Tech Stuff 🧰

***

In this section, you can showcase your skills and list the tools and technologies that you’re familiar with. I always prefer **minimal and crisp**design choices over cluttered data so I went ahead with icons this time as well. Us humans prefer information through visual mediums much more than anything else, right?

## Get Piyush Malhotra’s stories in your inbox

Join Medium for free to get updates from this writer.

Subscribe

Subscribe

You can use all of the stuff that I mentioned above in step 2 like [**IconFinder**](https://www.iconfinder.com/), [**Shields.io**](https://shields.io/), [**markdown-badges**](https://github.com/Ileriayo/markdown-badges), [**vector-logo-zone**](https://www.vectorlogo.zone/index.html), [**simple-icons**](https://simpleicons.org/), etc. But for this section, I would personally recommend [**DevIcon**](https://devicon.dev/). Unlike other resources, DevIcon is built for providing the icons solely related to programming languages and development tools which makes it a perfect fit.

Press enter or click to view image in full size

![Image 9](https://miro.medium.com/v2/resize:fit:700/1*ck4ZAQKAPreK2CFe7uBw7A.png)

[My GitHub Skills Section](https://github.com/thepiyushmalhotra)

Just copy the SVG image source from DevIcon’s website and paste it inside a <\*\*p\*\*> tag to show multiple icons!

## 🚀  Some Tools I Have Used and Learned

![vscode]([https://cdn.jsdelivr.net/gh/devicons/devicon/icons/vscode/vscode-original.svg](https://cdn.jsdelivr.net/gh/devicons/devicon/icons/vscode/vscode-original.svg)) ![bash]([https://cdn.jsdelivr.net/gh/devicons/devicon/icons/bash/bash-original.svg](https://cdn.jsdelivr.net/gh/devicons/devicon/icons/bash/bash-original.svg)) ![php]([https://cdn.jsdelivr.net/gh/devicons/devicon/icons/php/php-original.svg](https://cdn.jsdelivr.net/gh/devicons/devicon/icons/php/php-original.svg))

5.  Your GitHub History 📈

***

Finally, at the end of your Profile README, you can practically include anything. Some developers put up what’s currently playing on their [**Spotify profile**](https://github.com/kittinan/spotify-github-profile), some add their [**GitHub stats**](https://github.com/anuraghazra/github-readme-stats) or some add a fun little snake game on your GitHub contribution graph like me which I’ll show you guys how to put up!

Press enter or click to view image in full size

![Image 10](https://miro.medium.com/v2/resize:fit:700/1*D_gkcGSi6tO4o_dKVHcRow.gif)

[**My GitHub Stats!**](https://github.com/thepiyushmalhotra)

I begin with two GitHub ReadMe Stat Cards. One shows my total number of stars, commits and pull requests, etc. And the other one displays my most used Programming languages in percentages. You guys can get these cards from the popular [**GitHub ReadMe Stats Repo**](https://github.com/anuraghazra/github-readme-stats) and the best part about these is that they are fully customizable with different settings and themes!

Next up is probably my favorite thing out of all of my profile elements. Making a **Snake Game out of your GitHub Contribution Graph**. It’s fairly easy to set up and looks extremely satisfying when the snake gobbles up your commit graph.

To set it up for your profile, we are going to use something called GitHub Actions. GitHub Actions are CI/CD tools in GitHub where you can initiate workflows that automatically run, deploy and build your stuff.

![Snake animation](%5Bhttps://github.com/thepiyushmalhotra/thepiyushmalhotra/blob/output/github-contribution-grid-snake.svg%5D(https://github.com/thepiyushmalhotra/thepiyushmalhotra/blob/output/github-contribution-grid-snake.svg))

*   The first step is to copy this line above and add it to your profile’s README. Make sure to change the username to yours instead of mine.
*   Now we need to create a GitHub workflow so that the contribution graph in the snake animation will get updated according to the cronjob that we will set up.
*   Go to your “**_Actions_**” tab in your README repository and create a New Workflow. This will generate a new folder in your repository called “**_.github/workflows_**” and after that, it will make a new file inside of it called “**_main.yml_**”.

Press enter or click to view image in full size

![Image 11](https://miro.medium.com/v2/resize:fit:700/1*WzLn--jNcIuUTN8Y1QfDZA.png)

Setup a New Workflow

Press enter or click to view image in full size

![Image 12](https://miro.medium.com/v2/resize:fit:700/1*8b05PnlQ2XGSnpt7ZY0aug.png)

main.yml file

*   Delete everything inside of the newly created main.yml file and add this code to it below:

name: Generate Datas

on:

schedule: # execute every 12 hours

*   cron: "\* \*/12 \* \* \*"

workflow\_dispatch:

jobs:

build:

name: Jobs to update datas

runs-on: ubuntu-latest

steps:

# Snake Animation

*   uses: Platane/snk@master

id: snake-gifwith:

github\_user\_name: thepiyushmalhotra

svg\_out\_path: dist/github-contribution-grid-snake.svg

*   uses: crazy-max/[ghaction-github-pages@v2.1.3](mailto:ghaction-github-pages@v2.1.3)

with:

target\_branch: output

build\_dir: dist

env:

GITHUB\_TOKEN: ${{ secrets.GITHUB\_TOKEN }}

*   Make sure to replace my username with yours. We use a \*\*_cronjob_\*\*here that updates every 12 hours so whenever you have a new commit, it's going to get added to your snake animation.
*   The final step is to go back to your “**_Actions_**” page of your README file, click the newly created workflow “**_Generate Datas_**” or any name that you give it, and click the “**_Run Workflow_**” button.

Press enter or click to view image in full size

![Image 13](https://miro.medium.com/v2/resize:fit:700/1*sirkSkcYLraQPm9wBKVJBA.png)

Run Your Workflow

Et voila! Your Snake GitHub Contribution Graph is active now. Enjoy watching that snake eat up your hard work! To wrap it all up, I just added another capsule render animation at the footer which looked nice to me as you can see in the GIF above.

I’ve also mentioned some extra collated list of resources for your individual design needs:

*   [**_Awesome-GitHub-Profile-Readme_**](https://github.com/abhisheknaiidu/awesome-github-profile-readme)— A list of amazing Readmes from many talented developers!
*   [**_GitHub Readme Generator_**](https://rahuldkjain.github.io/gh-profile-readme-generator/) — An easy way to quickly generate a basic design template for your profile.
*   Read up more on [**_GitHub Actions_**](https://docs.github.com/en/actions/learn-github-actions/understanding-github-actions) if you’re interested in automation.
*   Generate various [**_GitHub metrics_**](https://github.com/lowlighter/metrics) that can be embedded anywhere.
*   If you want a quick and simple way to create cronjobs and learn more about them, then [**_Crontab Guru_**](https://crontab.guru/) is a pretty good resource and you get to know the exact time that your workflow will run.

And that was it for my GitHub Profile Design. Hope you guys liked this blog post on **how to design an attractive GitHub Profile Readme that’s aesthetically pleasing as well as functional**. Let me know if I can make some additions or changes to this article or if I missed some other fun stuff on GitHub. I do plan to add a blog post workflow as well which can show your latest blog posts from Dev.to but I’ll leave that for another short article.