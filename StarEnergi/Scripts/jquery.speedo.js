(function($){
	$.fn.speedometer = function(options){
		var 
			defaults = {
				category: 'single',
				val: 0,
				valInner: 0,
				curVal: 0,
				curValInner: 0,
				base: '',
			},
			settings = $.extend({}, defaults, options);
			
		this.each(function(){
			var $this = $(this);			
			function getImageBig(value){
				var src_big;
				if(value == 0){
					src_big = '/content/image/speedomtr/timeline/big_0.png';
				}else if((value > 5)&&(value <= 15)){
					src_big ='/content/image/speedomtr/timeline/big_10.png';	
				}else if((value > 15)&&(value <= 25)){
					src_big ='/content/image/speedomtr/timeline/big_20.png';	
				}else if((value > 25)&&(value <= 35)){
					src_big ='/content/image/speedomtr/timeline/big_30.png';	
				}else if((value > 35)&&(value <= 45)){
					src_big ='/content/image/speedomtr/timeline/big_40.png';	
				}else if((value > 45)&&(value <= 55)){
					src_big ='/content/image/speedomtr/timeline/big_50.png';	
				}else if((value > 55)&&(value <= 65)){
					src_big ='/content/image/speedomtr/timeline/big_60.png';	
				}else if((value > 65)&&(value <= 75)){
					src_big ='/content/image/speedomtr/timeline/big_70.png';	
				}else if((value > 75)&&(value <= 85)){
					src_big ='/content/image/speedomtr/timeline/big_80.png';	
				}else if((value > 85)&&(value <= 95)){
					src_big ='/content/image/speedomtr/timeline/big_90.png';	
				}else if((value > 95)&&(value <= 100)){
					src_big ='/content/image/speedomtr/timeline/big_100.png';	
				}
				
				// alert($this.html());
				
				// $('#test').html(src_big);
				
				$this.find('img.big').attr('src',src_big);
			}
			
			function getImageSmall(value){
				var src_small;
                
				if(value == 0){
					src_small ='/content/image/speedomtr/timeline/small_0.png';
				}else if((value > 5)&&(value <= 15)){
					src_small ='/content/image/speedomtr/timeline/small_10.png';	
				}else if((value > 15)&&(value <= 25)){
					src_small ='/content/image/speedomtr/timeline/small_20.png';	
				}else if((value > 25)&&(value <= 35)){
					src_small ='/content/image/speedomtr/timeline/small_30.png';	
				}else if((value > 35)&&(value <= 45)){
					src_small ='/content/image/speedomtr/timeline/small_40.png';	
				}else if((value > 45)&&(value <= 55)){
					src_small ='/content/image/speedomtr/timeline/small_50.png';	
				}else if((value > 55)&&(value <= 65)){
					src_small ='/content/image/speedomtr/timeline/small_60.png';	
				}else if((value > 65)&&(value <= 75)){
					src_small ='/content/image/speedomtr/timeline/small_70.png';	
				}else if((value > 75)&&(value <= 85)){
					src_small ='/content/image/speedomtr/timeline/small_80.png';	
				}else if((value > 85)&&(value <= 95)){
					src_small ='/content/image/speedomtr/timeline/small_90.png';	
				}else if((value > 95)&&(value <= 100)){
					src_small ='/content/image/speedomtr/timeline/small_100.png';	
				}
				
				$this.find('img.small').attr('src',src_small);
			}			

			var time = setInterval(function(){
				getImageBig(settings.curVal);
				if(settings.curVal <= settings.val){
					settings.curVal++;
				}else{
					clearInterval();
				}
			},15);

			if(settings.category == 'double'){
				var timeInner = setInterval(function(){
					getImageSmall(settings.curValInner);
					if(settings.curValInner <= settings.valInner){
						settings.curValInner++;
					}else{
						clearInterval();
					}
				},15);
			}
		});
		
		return this;
	};
})(jQuery);