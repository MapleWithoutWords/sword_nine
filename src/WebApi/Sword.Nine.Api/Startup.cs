using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SAM.Common.APICommon.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wanna.EMS.Api.SettingConfig;

namespace NoRain.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //��ӿ�������
            services.AddCors(options =>
            {
                options.AddPolicy("default",

                    builder => builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                            .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                );
            });

            //ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SwordNine",
                    Description = "RESTful API for SwordNine",
                    //TermsOfService = "None",

                });
                //���ӳ���ļ�������ע��
                var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var libs = DependencyContext.Default.CompileLibraries
                .Where(lib => !lib.Serviceable && lib.Type != "package" && (lib.Name.Contains("Domain") || lib.Name.Contains("NoRain.Common"))).ToList();
                libs.ForEach(x =>
                {
                    var xmlPathModel = Path.Combine(AppContext.BaseDirectory, $"{x.Name}.xml");

                    c.IncludeXmlComments(xmlPathModel);
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                c.DocInclusionPredicate((docName, description) => true);
            });
            //services.Configure<TokenManagement>(Configuration.GetSection("TokenManagement"));


            //���ݿ����
            services.AddDbFactoryService(Configuration);
            //��������ļ�����
            services.AddSettingService(Configuration, this.GetType().Assembly.FullName);

            ///���dao��
            services.RegisterScoped(BasciSetting.RegisterDaoName);
            ///���service��
            services.RegisterScoped(BasciSetting.RegisterServiceName);
            services.AddLogging();

            // �������ѹ��
            services.AddHttpClient();

            // ���ѹ�����
            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            // ���httpcontext����ע��
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<APIExceptionFilter>();
            //var serviceProvider = services.BuildServiceProvider();
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(APIResultFilter));
                option.Filters.Add(typeof(APIExceptionFilter));

            }).AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            //    .AddJsonOptions(option =>
            //{
            //    option.JsonSerializerOptions.Converters.Add(new StringToIntConvert());
            //    option.JsonSerializerOptions.Converters.Add(new StringToDoubleConvert());
            //    option.JsonSerializerOptions.Converters.Add(new StringToFloatConvert());
            //    option.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            //    option.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());
            //});

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenManagement.Secret)),
                    ValidIssuer = TokenManagement.Issuer,
                    ValidAudience = TokenManagement.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),//wwwroot�൱����ʵĿ¼
                //RequestPath = new PathString("/src") //src�൱�ڱ�����Ϊ�˰�ȫ
            });

            // ���ô���ѹ��
            app.UseResponseCompression();

            app.UseCors("default");
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwordNine API V1");
                c.DocExpansion(DocExpansion.None);
                c.DefaultModelsExpandDepth(-1);
            });


            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
