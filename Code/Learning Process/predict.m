function pred2 = predict(nbr_obj) 
	input=importdata('C:/Users/Sakher/Desktop/TC1 Apprentissage/input.txt');  %le descripteur a classifier
	load('training_data_100')
	prob2 = zeros(nbr_obj,numLabels);
	testLabel2=ones(nbr_obj,1)
	for k=1:numLabels
		[~,~,p2] = svmpredict(double(testLabel2==k), input, model{k}, '-b 1');
		prob2(:,k) = p2(:,model{k}.Label==1);    %# probability of class==k
	end
	[~,pred2] = max(prob2,[],2);
end